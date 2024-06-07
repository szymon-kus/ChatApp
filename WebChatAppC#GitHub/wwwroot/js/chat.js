document.addEventListener('DOMContentLoaded', function () {
    const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    let selectedUser = "";
    let loggedInUser = document.getElementById('loggedInUser').value;

    connection.start().then(function () {
        console.log("SignalR Connected.");
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

    document.querySelectorAll(".user-link").forEach(function (element) {
        element.addEventListener("click", function (event) {
            const username = event.target.dataset.username;
            if (username) {
                selectedUser = username;
                document.getElementById("usernameText").innerText = "Chat with " + selectedUser;
            } else {
                selectedUser = "";
                document.getElementById("usernameText").innerText = "Group Chat";
            }
            document.getElementById("messageList").innerHTML = "";
            loadMessages();
            event.preventDefault();
        });
    });

    connection.on("ReceiveMessage", function (user, message) {
        if (!selectedUser) {
            displayMessage(`${user}: ${message}`);
        }
    });

    connection.on("ReceivePrivateMessage", function (user, message) {
        if (user === selectedUser || user === loggedInUser) {
            displayMessage(`${user}: ${message}`);
        }
    });

    connection.on("ReceiveNotification", function (notificationMessage) {
        displayNotification(notificationMessage);
    });

    function sendMessage() {
        let messageInput = document.getElementById('messageInput').value;

        if (messageInput.trim() !== '') {
            if (selectedUser) {
                connection.invoke("SendPrivateMessage", loggedInUser, selectedUser, messageInput).catch(function (err) {
                    return console.error(err.toString());
                });
            } else {
                connection.invoke("SendMessage", loggedInUser, messageInput).catch(function (err) {
                    return console.error(err.toString());
                });
            }
            document.getElementById('messageInput').value = '';
        }
    }

    function displayMessage(message) {
        let messageList = document.getElementById('messageList');
        let messageItem = document.createElement('li');
        messageItem.classList.add("chatMessage");
        messageItem.textContent = message;
        messageList.appendChild(messageItem);
        messageList.scrollTop = messageList.scrollHeight;
    }

    function displayNotification(message) {
        let notificationArea = document.getElementById('notificationArea');
        let notification = document.createElement('div');
        notification.classList.add('notification');
        notification.textContent = message;

        notificationArea.appendChild(notification);
        notificationArea.style.display = 'block';

        setTimeout(() => {
            notificationArea.removeChild(notification);
            if (notificationArea.childElementCount === 0) {
                notificationArea.style.display = 'none';
            }
        }, 5000);
    }

    function loadMessages() {
        fetch('/Home/ChatMessages')
            .then(response => response.json())
            .then(messages => {
                document.getElementById('messageList').innerHTML = '';
                messages.forEach(message => {
                    if (message.receiver === 'Group' && !selectedUser) {
                        displayMessage(`${message.sender}: ${message.content}`);
                    } else if ((message.sender === loggedInUser && message.receiver === selectedUser) ||
                        (message.sender === selectedUser && message.receiver === loggedInUser)) {
                        displayMessage(`${message.sender} (private): ${message.content}`);
                    }
                });
            })
            .catch(error => console.error('Error loading messages:', error));
    }
});
