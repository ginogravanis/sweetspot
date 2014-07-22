using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using Microsoft.Kinect;
using Microsoft.Xna.Framework;

namespace Sweetspot.Input
{
    public enum SensorName { One, Two }

    public class SensorManager
    {
        const string RECORDING_PATH = "Recordings";
        /// <summary>
        /// The position the user should optimally be standing relative to the sensor.
        /// The x-coordinate corresponds to the user's position parallel to the sensor,
        /// 0 being the center of the sensor and positive coordinates going to the right of the sensor.
        /// The y-coordinate corresponds to the user's position away from the sensor,
        /// 0 being the surface of the sensor.
        /// </summary>
        public Vector2 sweetspot { get; set; }

        /// <summary>
        /// The minumum interaction distance from the sweetspot in meters.
        /// </summary>
        public float sweetspotMargin = 2f;

        /// <summary>
        /// The radius of the sweetspot in meters.
        /// </summary>
        public float sweetspotRadius = 0.1f;

        TimeSpan positionSmoothingTime = TimeSpan.FromMilliseconds(50);

        List<Sensor> sensors;
        Vector2 lastViewerPosition;
        TimeSpan viewerLastSeen;
        bool viewerActive;

        public static int MAX_SENSOR_COUNT = 2;
        public static float SENSOR_RANGE = 5.0f;
        protected int counter;
        protected bool record = false;
        protected int gameId;

        public SensorManager(ICalibrationProvider calibrationProvider)
        {
            sensors = new List<Sensor>();
            sweetspot = new Vector2();
            lastViewerPosition = new Vector2();
            viewerLastSeen = TimeSpan.FromSeconds(-1);
            viewerActive = false;
            initializeSensors(calibrationProvider);
            Directory.CreateDirectory(RECORDING_PATH);

        }

        protected void initializeSensors(ICalibrationProvider calibrationProvider)
        {
            foreach (KinectSensor candidate in KinectSensor.KinectSensors)
            {
                if (candidate.Status == KinectStatus.Connected)
                {
                    Sensor sensor = new Sensor(candidate, calibrationProvider);
                    sensor.Initialize();
                    sensors.Add(sensor);
                }
            }

            int maxSensors = SensorManager.MAX_SENSOR_COUNT;
            if (sensors.Count > maxSensors)
            {
                sensors.RemoveRange(maxSensors, sensors.Count - maxSensors);
            }
        }

        public void Update(GameTime gameTime)
        {
            foreach (Sensor sensor in sensors)
                sensor.Update(gameTime);

            if (viewerPositionAvailable())
            {
                lastViewerPosition = calculateViewerPosition();
                viewerLastSeen = gameTime.TotalGameTime;
                viewerActive = true;
            }
            else if (viewerRecentlySeen(gameTime))
                viewerActive = true;
            else
                viewerActive = false;

            if (record) {
                foreach (var sensor in sensors)
                    writeMultipleFramesToDisk(computeSensorPath(sensor), sensor.GetNextFrames()); 
            }
        }

        public bool IsViewerActive()
        {
            return viewerActive;
        }

        public Vector2 GetViewerPosition()
        {
            return lastViewerPosition;
        }

        protected bool viewerPositionAvailable()
        {
            foreach (Sensor sensor in sensors)
            {
                if (sensor.HasActiveUsers())
                    return true;
            }
            return false;
        }

        protected bool viewerRecentlySeen(GameTime gameTime)
        {
            return viewerLastSeen > gameTime.TotalGameTime - positionSmoothingTime;
        }

        protected Vector2 calculateViewerPosition()
        {
            if (sensors.Count == 2
                && sensors[0].GetActiveUserCount() == 1
                && sensors[1].GetActiveUserCount() == 1)
            {
                Vector2 position1 = sensors[0].GetUserPositions().First<Vector2>();
                Vector2 position2 = sensors[1].GetUserPositions().First<Vector2>();
                return Calibrator.CalculateMidpoint(position1, position2);
            }
            else
            {
                List<Vector2> positions = new List<Vector2>();
                foreach (Sensor sensor in sensors)
                {
                    positions.AddRange(sensor.GetUserPositions());
                }
                return getNearestViewer(positions);
            }
        }

        protected Vector2 getNearestViewer(List<Vector2> positions)
        {
            if (positions.Count == 0)
                throw new ApplicationException("User position not available.");

            float shortestDistanceToSweetspot = distanceToSweetspot(positions[0]);
            Vector2 nearestUserPosition = positions[0];
            foreach (Vector2 position in positions.Skip(1))
            {
                float newDistance = distanceToSweetspot(position);
                if (newDistance < shortestDistanceToSweetspot)
                {
                    shortestDistanceToSweetspot = newDistance;
                    nearestUserPosition = position;
                }
            }

            return nearestUserPosition;
        }

        public Vector2 GetVectorToSweetspot()
        {
            return vectorToSweetspot(lastViewerPosition);
        }

        protected Vector2 vectorToSweetspot(Vector2 position)
        {
            return sweetspot - position;
        }

        public float GetDistanceFromSweetspot()
        {
            return distanceToSweetspot(lastViewerPosition);
        }

        protected float distanceToSweetspot(Vector2 position)
        {
            float rawDistance = Math.Abs((sweetspot - position).Length());
            float distance = Math.Max(rawDistance - sweetspotRadius, 0);
            return distance;
        }

        public Sensor GetSensor(SensorName name)
        {
            if (sensors.Count == 0)
                throw new ApplicationException("No sensor connected!");

            Sensor sensorOne = sensors.First<Sensor>();
            Sensor sensorTwo = sensors.Last<Sensor>();
            Sensor result = sensorOne;

            if (sensors.Count > 1 && name == SensorName.Two)
            {
                result = sensorTwo;
            }

            return result;
        }

        public void ResetSensorTilt()
        {
            foreach (Sensor sensor in sensors)
            {
                sensor.ResetSensorTilt();
            }
        }

        public static Vector2 WorldToScreenCoords(Rectangle bounds, Vector2 position)
        {
            float x = bounds.Left + (bounds.Width / 2) + ((bounds.Width / 2f) * position.X / (SENSOR_RANGE / 2f));
            float y = bounds.Top + bounds.Height * (position.Y / SENSOR_RANGE);
            return new Vector2((int)Math.Round(x), (int)Math.Round(y));
        }

        protected void writeFrameToDisk(string folder, WriteableBitmap bitmap, DateTime time)
        {
            BitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            string formattedTime = time.ToString("hh'-'mm'-'ss");
            string filename = Path.Combine(folder, formattedTime + " - " + counter + ".png");
            counter++;

            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Create))
                {
                    encoder.Save(fs);
                }
            }
            catch (IOException)
            {
                Logger.Log("Can't save depth image.");
            }
        }

        protected void writeMultipleFramesToDisk(string folder, IEnumerable<Tuple<WriteableBitmap, DateTime>> frames)
        {
            foreach (var frame in frames)
                writeFrameToDisk(folder, frame.Item1, frame.Item2);
        }

        protected string computeGamePath()
        {
            return Path.Combine(RECORDING_PATH, "Game " + gameId);
        }

        protected string computeSensorPath(Sensor sensor)
        {
             return Path.Combine(computeGamePath(), sensor.ToString());
        }

        public void StartRecording(int gameId) 
        {
            record = true;
            this.gameId = gameId;

            foreach (Sensor sensor in sensors)
            {
                string sensorPath = Path.Combine(computeGamePath(), sensor.ToString());
                Directory.CreateDirectory(sensorPath);
            }
        }

        public void StopRecording()
        {
            record = false;
            foreach (Sensor sensor in sensors)
            {
                writeMultipleFramesToDisk(computeSensorPath(sensor), sensor.GetRemainingFrames());
            }
        }
    }
}
