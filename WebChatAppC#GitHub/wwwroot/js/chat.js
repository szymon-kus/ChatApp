function sendMessage() {
    var messageInput = document.getElementById('messageInput');
    var chatMessages = document.getElementById('chatMessages');
    var loggedInUser = document.getElementById('loggedInUser').value;

    if (messageInput.value.trim() !== '') {
        var messageElement = document.createElement('div');
        messageElement.innerHTML = `<strong>${loggedInUser}:</strong> ${messageInput.value}`;
        chatMessages.appendChild(messageElement);

        messageInput.value = '';
    }
}
