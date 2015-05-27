/*
 *
 * Copyright (C) Carnegie Mellon University - All Rights Reserved.
 * Unauthorized copying of this file, via any medium is strictly prohibited.
 * This is proprietary and confidential.
 * Written by members of the ArticuLab, directed by Justine Cassell, 2014.
 * 
 * Author: Alexandros Papangelis, apapa@cs.cmu.edu
 * 
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace InMind
{
    class Utilities
    {
        public static int maxInd(double[] matrix, int size)
        {
            if (size <= 0) { return -10000000; }
            else if (size == 1) { return 0; }

            double _max = matrix[0];
            int pos = 0;

            for (int i = 1; i < size; i++)
            {
                if (_max < matrix[i])
                {
                    pos = i;
                    _max = matrix[i];
                }
            }

            return pos;
        }

        //Version with known maximum - to handle multiple maxima inside matrix
        public static int maxInd(double[] matrix, double knownMax, int size)
        {
            if (size <= 0) { return -10000000; }
            else if (size == 1) { return 0; }

            List<int> max_positions = new List<int>();
            Random rand = new Random();

            for (int i = 0; i < size; i++)
            {
                if (knownMax == matrix[i])
                {
                    max_positions.Add(i);
                }
            }

            //knownMax may not be in the matrix. if matrix represents a
            //Reinforcement Learning policy vector for a specific state for example,
            //it may be the case that the situation has changed and the entry
            //with the maximum utility no longer has that value (but a lower one)
            //and no other entry has that value as well.
            if (max_positions.Count > 0)
            {
                return max_positions[rand.Next(max_positions.Count - 1)];
            }
            else { return maxInd(matrix, size); }
        }

        public static int max(int[] matrix, int size)
        {
            if (size <= 0) { return -10000000; }
            else if (size == 1) { return matrix[0]; }

            int _max = matrix[0];

            for (int i = 1; i < size; i++)
            {
                _max = matrix[i] > _max ? matrix[i] : _max;
            }

            return _max;
        }

        public static double max(double[] matrix, int size)
        {
            if (size <= 0) { return -10000000; }
            else if (size == 1) { return matrix[0]; }

            double _max = matrix[0];

            for (int i = 1; i < size; i++)
            {
                _max = matrix[i] > _max ? matrix[i] : _max;
            }

            return _max;
        }

        //REF: http://stackoverflow.com/questions/6115721/how-to-save-restore-serializable-object-to-from-file
        /// <summary>
        /// Serializes an object.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serializableObject"></param>
        /// <param name="fileName"></param>
        public static void SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return; }

            try
            {
                XmlDocument xmlDocument = new XmlDocument();
                XmlSerializer serializer = new XmlSerializer(serializableObject.GetType());
                using (MemoryStream stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileName);
                    stream.Close();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }
        }


        //REF: http://stackoverflow.com/questions/6115721/how-to-save-restore-serializable-object-to-from-file
        /// <summary>
        /// Deserializes an xml file into an object list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeSerializeObject<T>(string fileName)
        {
            if (string.IsNullOrEmpty(fileName)) { return default(T); }

            T objectOut = default(T);

            try
            {
                string attributeXml = string.Empty;

                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(fileName);
                string xmlString = xmlDocument.OuterXml;

                using (StringReader read = new StringReader(xmlString))
                {
                    Type outType = typeof(T);

                    XmlSerializer serializer = new XmlSerializer(outType);
                    using (XmlReader reader = new XmlTextReader(read))
                    {
                        objectOut = (T)serializer.Deserialize(reader);
                        reader.Close();
                    }

                    read.Close();
                }
            }
            catch (Exception ex)
            {
                //Log exception here
            }

            return objectOut;
        }
    }
}
