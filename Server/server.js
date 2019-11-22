var io = require('socket.io')({
	transports: ['websocket'],
});

io.attach(8080);

io.on('connection', function(socket){
	console.log("client connexion");
	socket.emit("boop");
	socket.on('beep', function(socket){
		console.log("recu beep");
		//socket.emit('boop');
	});
	
})



