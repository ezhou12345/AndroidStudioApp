/*
 *
 * Copyright (C) Carnegie Mellon University - All Rights Reserved.
 * Unauthorized copying of this file, via any medium is strictly prohibited.
 * This is proprietary and confidential.
 * Written by members of the ArticuLab, directed by Justine Cassell, 2014.
 * 
 * 
 * Author: Alexandros Papangelis, apapa@cs.cmu.edu
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace vrGlobalData
{
    public class UserModel
    {
        private int _data;  //for testing only, to be removed

        private SharedKnowledge _shortTermKnowledge = new SharedKnowledge(), _longTermKnowledge = new SharedKnowledge();
        private LearningModel _learningModel = new LearningModel();
        private VirtualHumanModel _putativeVHState = new VirtualHumanModel();
        private Dictionary<string, string> _personalInformation = new Dictionary<string, string>();
        private bool _isFriend; //The user is our friend (true) or not (false)

        public UserModel() { _data = 0; }

        ~UserModel() { }

        public void setData(int data){
            _data = data;
            Console.WriteLine(" userModel Updated: " + _data);
        }

        public int getdata() {
            return _data;
        }

        public bool isFriend() { return _isFriend; }

        public String toString(){
            return _data.ToString();
        }

        //REF: http://stackoverflow.com/questions/11447529/convert-an-object-to-an-xml-string
        public string ToXML()
        {
            var stringwriter = new System.IO.StringWriter();
            var serializer = new XmlSerializer(this.GetType());
            serializer.Serialize(stringwriter, this);
            return stringwriter.ToString();
        }

        //REF: http://stackoverflow.com/questions/11447529/convert-an-object-to-an-xml-string
        public static UserModel LoadFromXMLString(string xmlText)
        {
            var stringReader = new System.IO.StringReader(xmlText);
            var serializer = new XmlSerializer(typeof(UserModel));
            return serializer.Deserialize(stringReader) as UserModel;
        }
    }

    class SharedKnowledge { }

    class LearningModel { }

    class VirtualHumanModel { }
}
