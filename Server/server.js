var express = require('express');
var session = require('express-session');
var app = express();

var session = require("express-session")({
    secret: "my-secret",
    resave: true,
    saveUninitialized: true
});
var sharedsession = require("express-socket.io-session");

var lineReader = require('readline').createInterface({
  input: require('fs').createReadStream('Questions.txt')
});

app.use(session);



// la base
var io = require('socket.io')({
	transports: ['websocket'],
});

io.use(sharedsession(session, {
    autoSave:true
}));

io.attach(8080);


// include our files
var tools = require('./Game');
global.playerNumber = 0;
global.numberQuestion = 0;

global.scorePlayer = new Map();
//maMap.set(chaineClé, "valeur associée à 'une chaîne'");
//maMap.get(chaineClé);

tools.Main(io);


console.log("sa tourne au calme");

i =0;
global.questions = [];

lineReader.on("line", function (line) {	
	
	question = {};

	answer = {};
	questionAndResponses = line.split("//")[0];
	indexResponse = line.split("//")[1];
	
	questionTitle = questionAndResponses.split("#&")[0];
	responses = questionAndResponses.split("#&")[1];
	responsesString = responses.split("/");
	for(j = 0 ; j < responsesString.length ; j++){
		response = responses.split("/")[j];
		answer[j] =  response;
	}
	
	question = {"title" : questionTitle,"goodAnswer" : indexResponse ,answer };
	
	questions.push(question);
	i++;
});



