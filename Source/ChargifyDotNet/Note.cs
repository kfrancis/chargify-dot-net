
#region License, Terms and Conditions
//
// Adjustment.cs
//
// Authors: Kori Francis <twitter.com/djbyter>, David Ball
// Copyright (C) 2011 Clinical Support Systems, Inc. All rights reserved.
// 
//  THIS FILE IS LICENSED UNDER THE MIT LICENSE AS OUTLINED IMMEDIATELY BELOW:
//
//  Permission is hereby granted, free of charge, to any person obtaining a
//  copy of this software and associated documentation files (the "Software"),
//  to deal in the Software without restriction, including without limitation
//  the rights to use, copy, modify, merge, publish, distribute, sublicense,
//  and/or sell copies of the Software, and to permit persons to whom the
//  Software is furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
//  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
//  DEALINGS IN THE SOFTWARE.
//
#endregion

namespace ChargifyNET
{
    #region Imports
    using System;
    using System.Xml;
    using Json;
    #endregion

    /// <summary>
    /// https://docs.chargify.com/api-notes
    /// </summary>
    public class Note : ChargifyBase, INote, IComparable<Note>
    {
        #region Field Keys
        private const string IdKey = "id";
        private const string BodyKey = "body";
        private const string StickyKey = "sticky";
        private const string UpdatedAtKey = "updated_at";
        private const string SubscriptionIdKey = "subscription_id";
        private const string CreatedAtKey = "created_at";
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.  Values set to default
        /// </summary>
        public Note()
        { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="noteXml">XML containing note info (in expected format)</param>
        public Note(string noteXml)
        {
            // get the XML into an XML document
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(noteXml);
            if (doc.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(noteXml));
            // loop through the child nodes of this node
            foreach (XmlNode elementNode in doc.ChildNodes)
            {
                if (elementNode.Name == "note")
                {
                    LoadFromNode(elementNode);
                    return;
                }
            }
            // if we get here, then no info was found
            throw new ArgumentException("XML does not contain note information", nameof(noteXml));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="noteNode">XML containing note info (in expected format)</param>
        internal Note(XmlNode noteNode)
        {
            if (noteNode == null) throw new ArgumentNullException(nameof(noteNode));
            if (noteNode.Name != "note") throw new ArgumentException("Not a vaild note node", nameof(noteNode));
            if (noteNode.ChildNodes.Count == 0) throw new ArgumentException("XML not valid", nameof(noteNode));
            LoadFromNode(noteNode);
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="noteObject">Json containing note info (in expected format)</param>
        public Note(JsonObject noteObject)
        {
            if (noteObject == null) throw new ArgumentNullException(nameof(noteObject));
            if (noteObject.Keys.Count <= 0) throw new ArgumentException("Not a vaild note object", nameof(noteObject));
            LoadFromJson(noteObject);
        }

        private void LoadFromJson(JsonObject obj)
        {
            foreach (string key in obj.Keys)
            {
                switch (key)
                {
                    case IdKey:
                        _id = obj.GetJSONContentAsInt(key);
                        break;
                    case BodyKey:
                        Body = obj.GetJSONContentAsString(key);
                        break;
                    case SubscriptionIdKey:
                        SubscriptionID = obj.GetJSONContentAsInt(key);
                        break;
                    case CreatedAtKey:
                        _createdAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case UpdatedAtKey:
                        _updatedAt = obj.GetJSONContentAsDateTime(key);
                        break;
                    case StickyKey:
                        Sticky = obj.GetJSONContentAsBoolean(key);
                        break;
                }
            }
        }

        /// <summary>
        /// Load data from a note node
        /// </summary>
        /// <param name="noteNode">The note node</param>
        private void LoadFromNode(XmlNode noteNode)
        {
            foreach (XmlNode dataNode in noteNode.ChildNodes)
            {
                switch (dataNode.Name)
                {
                    case IdKey:
                        _id = dataNode.GetNodeContentAsInt();
                        break;
                    case BodyKey:
                        Body = dataNode.GetNodeContentAsString();
                        break;
                    case SubscriptionIdKey:
                        SubscriptionID = dataNode.GetNodeContentAsInt();
                        break;
                    case CreatedAtKey:
                        _createdAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case UpdatedAtKey:
                        _updatedAt = dataNode.GetNodeContentAsDateTime();
                        break;
                    case StickyKey:
                        Sticky = dataNode.GetNodeContentAsBoolean();
                        break;
                }
            }
        }
        #endregion

        #region INote Members
        /// <summary>
        /// The main text content of the note
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Date and time the note was created
        /// </summary>
        public DateTime CreatedAt
        {
            get
            {
                return _createdAt;
            }
        }
        private DateTime _createdAt = DateTime.MinValue;

        /// <summary>
        /// The note's unique id
        /// </summary>
        public int ID
        {
            get
            {
                return _id;
            }
        }
        private int _id = int.MinValue;

        /// <summary>
        /// Whether or not it is pinned to the top of the list of notes
        /// </summary>
        public bool Sticky { get; set; }

        /// <summary>
        /// The id of the related subscription
        /// </summary>
        public int SubscriptionID { get; set; }

        /// <summary>
        /// Last update timestamp
        /// </summary>
        public DateTime UpdatedAt
        {
            get
            {
                return _updatedAt;
            }
        }
        private DateTime _updatedAt = DateTime.MinValue;
        #endregion

        #region IComparable<INote> Implementation
        /// <summary>
        /// Method for comparing two notes
        /// </summary>
        /// <param name="other">The note to compare with</param>
        /// <returns>The comparison value</returns>
        public int CompareTo(INote other)
        {
            return ID.CompareTo(other.ID);
        }
        #endregion

        #region IComparable<Note> Implementation
        /// <summary>
        /// Method for comparing two notes
        /// </summary>
        /// <param name="other">The note to compare with</param>
        /// <returns>The comparison value</returns>
        public int CompareTo(Note other)
        {
            return ID.CompareTo(other.ID);
        }
        #endregion
    }
}
