module.exports = {

	Main : function(io){
		io.on('connection', function(socket){
			join(socket);
		});
	},
};


function join(socket){

	socket.on('join', function(data ){
		
		playerNumber++;
		idPlayer = playerNumber;
		namePlayer = data.name;
		
		// session
		socket.broadcast.emit("joinAll",{id : idPlayer,name : namePlayer});
		socket.emit("join",{id : idPlayer, nbPlayer : playerNumber} );
		scorePlayer.set(idPlayer, 0);
		
		console.log("player join : " + idPlayer + "  " +namePlayer);
		
		/*socket.on('beep', function(){
		socket.handshake.session.userdata = new Map();
		socket.handshake.session.userdata(
		socket.handshake.session.save();
		socket.emit('boop',{p{name : "homertimes"}seudo : socket.handshake.session.userdata});
		});*/
	});


	socket.on('getQuestions', function(data ){
		socket.emit("getQuestions",{questions} )
	})
	
	
	socket.on('getCurrentQuestion', function(data ){
		socket.emit("getCurrentQuestion",{question : numberQuestion} )
		numberQuestion++;
	})
	
	socket.on('setReponse', function(data ){
		idPlayer = data.id;
		answer = data.answer;
		goodAnswer = 0;
		currentScore = scorePlayer.get(idPlayer);
		
		console.log(questions[numberQuestion].goodAnswer);
		console.log(answer);
		if(questions[numberQuestion].goodAnswer == answer){
			goodAnswer = 1;
			currentScore++;
			scorePlayer.set(idPlayer, currentScore);
		}
		
		socket.emit("setReponse",{id : idPlayer, answer : goodAnswer} );
	})


	socket.on('getScore', function(data ){
		scoreJSON  = [];
		var i;
		scorePlayer.forEach(function(value,key, map){
			scoreJSON.push({id : key, score : value});
		})
		console.log(scorePlayer);
		socket.emit("getScore", {scoreJSON});
	})
}