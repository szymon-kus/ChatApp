document.addEventListener('DOMContentLoaded', function () {
    const connection = new signalR.HubConnectionBuilder().withUrl("/chatHub").build();
    let selectedUser = "";

    connection.start().then(function () {
        console.log("SignalR Connected.");
        document.getElementById("sendButton").disabled = false;
        loadMessages(); 
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
        if (!selectedUser || user === selectedUser) {
            displayMessage(`${user}: ${message}`);
        }
    });

    connection.on("ReceivePrivateMessage", function (user, message) {
        if (user === selectedUser || user === document.getElementById("loggedInUser").value) {
            displayMessage(`${user} (private): ${message}`);
        }
    });

    function sendMessage() {
        let messageInput = document.getElementById('messageInput').value;
        let loggedInUser = document.getElementById('loggedInUser').value;

        if (messageInput.trim() !== '') {
            if (selectedUser) {
                connection.invoke("SendPrivateMessage", loggedInUser, selectedUser, messageInput).catch(function (err) {
                    return console.error(err.toString());
                });
                displayMessage(`${loggedInUser} (private): ${messageInput}`);
            } else {
                connection.invoke("SendMessage", loggedInUser, messageInput).catch(function (err) {
                    return console.error(err.toString());
                });
                displayMessage(`${loggedInUser}: ${messageInput}`);
            }
            document.getElementById('messageInput').value = '';
        }
    }

    function displayMessage(message) {
        let messageList = document.getElementById('messageList');
        let messageItem = document.createElement('li');
        messageItem.textContent = message;
        messageItem.classList.add('chatMessage');
        messageList.appendChild(messageItem);
    }

    function loadMessages() {
        const loggedInUser = document.getElementById('loggedInUser').value;

        fetch('/Home/ChatMessages')
            .then(response => response.json())
            .then(messages => {
                messages.forEach(message => {
                    if ((message.receiver === 'Group' && !selectedUser) || 
                        (message.sender === loggedInUser && message.receiver === selectedUser) ||
                        (message.sender === selectedUser && message.receiver === loggedInUser)) {
                        displayMessage(`${message.sender}: ${message.content}`);
                    }
                });
            })
            .catch(error => console.error('Error loading messages:', error));
    }

    //loadMssages() zwarijujejejsuej zaraz
});
