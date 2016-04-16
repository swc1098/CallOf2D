var app = require('http').createServer(handler)
var io = require('socket.io')(app);
var lockstepio = require('./lockstep.io.js')(io);
var port = process.env.PORT || process.env.NODE_PORT || 3000;

app.listen(port);
console.log('Lockstep.IO: Listening on port ' + port + '!');
//console.log('Connect to Unity locally with the following URL:');
//console.log('ws://127.0.0.1:' + port + '/socket.io/?EIO=4&transport=websocket');


function handler(req, res) {
    res.writeHead(200);
    res.end('<3');
};