using System;
using ChargifyNET;
using ChargifyDotNetTests.Base;
using System.Linq;
using ChargifyDotNet.Tests;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ChargifyDotNetTests
{
    [TestClass]
    public class NoteTests : ChargifyTestBase
    {
        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Notes_CanCreate(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList(SubscriptionState.Active).FirstOrDefault().Value as Subscription;
            Assert.IsNotNull(subscription, "No suitable subscription could be found.");
            var noteBody = Guid.NewGuid().ToString();
            Random rand = new();
            var isSticky = rand.Next(0, 1) == 1;

            // Act
            var result = Chargify.CreateNote(subscription.SubscriptionID, noteBody, isSticky);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(noteBody, result.Body);
            Assert.AreEqual(isSticky, result.Sticky);

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Notes_CanCopy(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subList = Chargify.GetSubscriptionList();
            var firstSubscription = subList.FirstOrDefault(s => s.Value.State == SubscriptionState.Active && Chargify.GetNotesForSubscription(s.Key) != null && Chargify.GetNotesForSubscription(s.Key).Any()).Value as Subscription;
            if (firstSubscription == null) Assert.Inconclusive("No suitable subscription could be found.");
            var note = Chargify.GetNotesForSubscription(firstSubscription.SubscriptionID).FirstOrDefault().Value;
            var secondSubscription = subList.FirstOrDefault(s => s.Value.State == SubscriptionState.Active && Chargify.GetNotesForSubscription(s.Key) == null || !Chargify.GetNotesForSubscription(s.Key).Any()).Value as Subscription;
            if (secondSubscription == null) Assert.Inconclusive("No suitable second subscription could be found.");
            note.SubscriptionID = secondSubscription.SubscriptionID;

            // Act
            var result = Chargify.CreateNote(note);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreNotEqual(firstSubscription.SubscriptionID, result.SubscriptionID);
            Assert.AreEqual(secondSubscription.SubscriptionID, result.SubscriptionID);

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Notes_CanLoad(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subList = Chargify.GetSubscriptionList();
            var subscription = subList.FirstOrDefault(s => s.Value.State == SubscriptionState.Active && Chargify.GetNotesForSubscription(s.Key) != null && Chargify.GetNotesForSubscription(s.Key).Any()).Value as Subscription;
            if (subscription == null) Assert.Inconclusive("No suitable subscription could be found.");
            var note = Chargify.GetNotesForSubscription(subscription.SubscriptionID).FirstOrDefault().Value;
            Assert.IsNotNull(note, "No suitable note could be found");

            // Act
            var result = Chargify.LoadNote(note.SubscriptionID, note.ID);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(note.ID, result.ID);
            Assert.AreEqual(note.Body, result.Body);
            Assert.AreEqual(note.Sticky, result.Sticky);
            Assert.AreEqual(note.CreatedAt, result.CreatedAt);
            Assert.AreEqual(note.UpdatedAt, result.UpdatedAt);

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Notes_CanGetForSubscription(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subList = Chargify.GetSubscriptionList();
            var firstSubscription = subList.FirstOrDefault(s => s.Value.State == SubscriptionState.Active && Chargify.GetNotesForSubscription(s.Key) != null && Chargify.GetNotesForSubscription(s.Key).Any()).Value as Subscription;
            if (firstSubscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var secondSubscription = subList.FirstOrDefault(s => s.Value.State == SubscriptionState.Active && Chargify.GetNotesForSubscription(s.Key) == null || !Chargify.GetNotesForSubscription(s.Key).Any()).Value as Subscription;
            if (secondSubscription == null) Assert.Inconclusive("A valid subscription could not be found.");

            // Act
            var firstResult = Chargify.GetNotesForSubscription(firstSubscription.SubscriptionID);
            var secondResult = Chargify.GetNotesForSubscription(secondSubscription.SubscriptionID);

            // Assert
            Assert.IsNotNull(firstResult);
            Assert.IsNotNull(secondResult);
            Assert.IsEmpty(secondResult);
            Assert.IsNotEmpty(firstResult);

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Notes_CanUpdate(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList(SubscriptionState.Active).FirstOrDefault(s => Chargify.GetNotesForSubscription(s.Key) != null && Chargify.GetNotesForSubscription(s.Key).Any()).Value as Subscription;
            if (subscription == null) Assert.Inconclusive("A valid subscription could not be found.");
            var notes = Chargify.GetNotesForSubscription(subscription.SubscriptionID);
            var note = notes.FirstOrDefault().Value;
            var updatedBody = Guid.NewGuid().ToString();
            note.Body = updatedBody;
            var updatedSticky = !note.Sticky;
            note.Sticky = updatedSticky;

            // Act
            var result = Chargify.UpdateNote(note.SubscriptionID, note);

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(updatedBody, result.Body);
            Assert.AreEqual(updatedSticky, result.Sticky);
            Assert.AreEqual(note.SubscriptionID, result.SubscriptionID);

            SetJson(!isJson);
        }

        
        [DataRow("xml")]
        [DataRow("json")]
        [TestMethod]
        public void Notes_CanDelete(string method)
        {
            var isJson = method == "json";
            SetJson(isJson);

            // Arrange
            var subscription = Chargify.GetSubscriptionList(SubscriptionState.Active).FirstOrDefault(s => Chargify.GetNotesForSubscription(s.Key) != null && Chargify.GetNotesForSubscription(s.Key).Any()).Value as Subscription;
            Assert.IsNotNull(subscription, "No suitable subscription could be found.");
            var notes = Chargify.GetNotesForSubscription(subscription.SubscriptionID);
            Assert.IsNotNull(notes.FirstOrDefault());

            // Act
            var result = Chargify.DeleteNote(subscription.SubscriptionID, notes.FirstOrDefault().Key);

            // Assert
            Assert.IsTrue(result);

            SetJson(!isJson);
        }
    }
}
