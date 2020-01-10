module.exports = {

	Main : function(io){
		io.on('connection', function(socket){
			join(socket);
		});
	},
};



function join(socket){
	
	socket.on('join', function(data ){
		
		idPlayer = playerNumber;
		playerNumber++;
		namePlayer = data.name;
		
		console.log(playerNumber);
		// session
		socket.broadcast.emit("joinAll",{id : idPlayer,name : namePlayer});
		scorePlayer.set(idPlayer, 0);
		console.log("dans le join current score : " +scorePlayer.get(idPlayer));
		namePlayerMap.set(idPlayer, namePlayer);
		console.log("player join : " + idPlayer + "  " +namePlayer);
		var namePlayerJson = setMapToJson(namePlayerMap);
		socket.emit("join",{id : idPlayer, nbPlayer : playerNumber,namePlayerJson} );
	});
	
	socket.on('resetVariables', function(data){
		playerNumber = 0;
		nbPlayerAnswer = 0;

		scorePlayer = new Map();
		namePlayerMap = new Map();
		nbJoueurGetScore=0;
	})

	socket.on('responded', function(data){
		console.log(data);
		socket.broadcast.emit("respondedd",{id : data.id} )
		socket.emit("respondedd",{id : data.id} )
	})

	socket.on('getQuestions', function(data ){
		socket.emit("getQuestions",{questions} )
		socket.broadcast.emit("getQuestions",{questions} )
	})
	
	
	socket.on('getCurrentQuestion', function(data ){
		console.log("le serveur a emit "+numberQuestion);
		socket.emit("getCurrentQuestion",{question : numberQuestion} )
	})
	
	
	socket.on('setReponse', function(data ){
		idPlayer = data.id;
		console.log("id du fdp de joueur : "+idPlayer);
		answer = data.answer;
		goodAnswer = 0;
		
		currentScore = scorePlayer.get(idPlayer);
		console.log("current score : "+scorePlayer.get(idPlayer));
		
		console.log("Question : " +questions);
		console.log("Number question : " +numberQuestion);
		console.log("good answer : " +questions[numberQuestion].goodAnswer);
		console.log("answer : " +answer);
		if(questions[numberQuestion].goodAnswer == answer){
			goodAnswer = 1;
			currentScore++;
			scorePlayer.set(idPlayer, currentScore);
		}
		socket.broadcast.emit("setReponse",{id : idPlayer, answer : goodAnswer} );
		socket.emit("setReponse",{id : idPlayer, answer : goodAnswer} );
		nbPlayerAnswer++;
		if(nbPlayerAnswer == playerNumber){
			numberQuestion++;
		}
	})


	socket.on('getScore', function(data ){
		scoreJSON  = [];
		var i;
		scorePlayer.forEach(function(value,key, map){
			scoreJSON.push({id : key, score : value});
		})
		console.log(scorePlayer.size + "le scoooooooooooooooooooooooooooooooooooooooree");
		socket.emit("getScore", {scoreJSON});
		//playerNumber = 0;
		//numberQuestion = -1;
		nbJoueurGetScore++;
		if(nbJoueurGetScore == playerNumber){
			scorePlayer.clear();
			namePlayerMap.clear();
		}

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