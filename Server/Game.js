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
		scorePlayer.set(idPlayer, 0);
		namePlayerMap.set(idPlayer, namePlayer);
		console.log("player join : " + idPlayer + "  " +namePlayer);
		var namePlayerJson = setMapToJson(namePlayerMap);
		socket.emit("join",{id : idPlayer, nbPlayer : playerNumber,namePlayerJson} );
	});

	socket.on('responded', function(data ){
		socket.broadcast.emit("responded",{id : data.id} );
		socket.emit("responded",{id : data.id} );
	})

	socket.on('getQuestions', function(data ){
		socket.emit("getQuestions",{questions} )
	})
	
	
	socket.on('getCurrentQuestion', function(data ){
		numberQuestion++;
		socket.emit("getCurrentQuestion",{question : numberQuestion} )
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
		socket.broadcast.emit("setReponse",{id : idPlayer, answer : goodAnswer} );
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
		playerNumber = 0;
		numberQuestion = -1;
		scorePlayer.clear();
		namePlayerMap.clear();
	})
	
}


function setMapToJson(map){
	objectJSON  = [];
	var i;
	map.forEach(function(value,key, map){
		objectJSON.push({id : key, name : value});
	})
	return objectJSON
}