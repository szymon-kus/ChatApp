"use strict";

// SignalR connection setup
var connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();

// Disable the send button until connection is established.
document.getElementById("sendButton").disabled = true;

connection.on("ReceiveMessage", function (user, message) {
    let chatMessagesContainer = document.getElementById('chatMessagesContainer');
    let chatMessage = document.createElement('div');
    chatMessage.classList.add("chatMessage");
    chatMessage.innerHTML = `<strong>${user}:</strong> ${message}`;
    chatMessagesContainer.appendChild(chatMessage);
});

connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("sendButton").addEventListener("click", function (event) {
    sendMessage();
    event.preventDefault();
});

document.getElementById("messageInput").addEventListener("keydown", function (event) {
    if (event.key === "Enter") {
        sendMessage();
        event.preventDefault();
    }
});

function sendMessage() {
    let loggedInUser = document.getElementById('loggedInUser').value;
    let sessionId = document.getElementById('sessionId').value;
    let messageInput = document.getElementById('messageInput').value;
    if (messageInput.trim() !== '') {
        connection.invoke("SendMessage", loggedInUser, messageInput).catch(function (err) {
            return console.error(err.toString());
        });
        document.getElementById('messageInput').value = '';
    }
}
