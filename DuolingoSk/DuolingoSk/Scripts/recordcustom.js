var recrdflnm = "";
var guid = "";
navigator.getUserMedia = navigator.getUserMedia || navigator.webkitGetUserMedia || navigator.mozGetUserMedia;
var arryblob = new Array();
function captureUserMedia(mediaConstraints, successCallback, errorCallback) {
    navigator.getUserMedia(mediaConstraints, successCallback, errorCallback);
    //navigator.mediaDevices.getUserMedia(mediaConstraints).then(successCallback).catch(errorCallback);
}

var mediaConstraints = {
    audio: true
};

function startrecrding() {
    guid = createGuid();
    fileName = Math.round(Math.random() * 99999999) + 99999999;
    recrdflnm = guid + fileName + ".wav";
    captureUserMedia(mediaConstraints, onMediaSuccess, onMediaError);
}

function stoprecrding() {  
    if (flgstopdone == true) {
        flgstopdone = false;
        $.blockUI();
        mediaRecorder.stop();
        mediaRecorder.stream.stop();
    }    
}


//document.querySelector('#pause-recording').onclick = function () {
//this.disabled = true;
//  mediaRecorder.pause();

///document.querySelector('#resume-recording').disabled = false;
//  };

// document.querySelector('#resume-recording').onclick = function () {
//  this.disabled = true;
//  mediaRecorder.resume();

//  document.querySelector('#pause-recording').disabled = false;
//  };


var mediaRecorder;

function onMediaSuccess(stream) {
    ///  var audio = document.createElement('audio');

    // audio = mergeProps(audio, {
    //  controls: true,
    // muted: true
    // });
    // audio.srcObject = stream;
    // audio.play();

    

    mediaRecorder = new MediaStreamRecorder(stream);
    mediaRecorder.stream = stream;

    var recorderType = 'WebAudio API (WAV)';

    if (recorderType === 'MediaRecorder API') {
        mediaRecorder.recorderType = MediaRecorderWrapper;
    }

    if (recorderType === 'WebAudio API (WAV)') {
        mediaRecorder.recorderType = StereoAudioRecorder;
        mediaRecorder.mimeType = 'audio/wav';
    }

    if (recorderType === 'WebAudio API (PCM)') {
        mediaRecorder.recorderType = StereoAudioRecorder;
        mediaRecorder.mimeType = 'audio/pcm';
    }

    // don't force any mimeType; use above "recorderType" instead.
    // mediaRecorder.mimeType = 'audio/webm'; // audio/ogg or audio/wav or audio/webm

    mediaRecorder.audioChannels = 1;// !!document.getElementById('left-channel').checked ? 1 : 2;
    mediaRecorder.ondataavailable = function (blob) {      
        var objblb = { recordflnm: recrdflnm, blobfl: blob, guidtxt: guid };
        arrblob.push(objblb);
        $.unblockUI();
        nextquestion();
      
    };

    var timeInterval11 = 600000; //document.querySelector('#time-interval').value;
    if (timeInterval11) timeInterval11 = parseInt(timeInterval11);
    else timeInterval11 = 5 * 1000;

    // get blob after specific time interval
    mediaRecorder.start(timeInterval11);
}

function onMediaError(e) {
    console.error('media error', e);
}

var index = 1;

window.onbeforeunload = function () {
    //document.querySelector('#start-recording').disabled = false;
};

function createGuid() {
    function S4() {
        return (((1 + Math.random()) * 0x10000) | 0).toString(16).substring(1);
    }
    return (S4() + S4() + "-" + S4() + "-4" + S4().substr(0, 3) + "-" + S4() + "-" + S4() + S4() + S4()).toLowerCase();
}
function getTimeLength(milliseconds) {
    var data = new Date(milliseconds);
    return data.getUTCHours() + " hours, " + data.getUTCMinutes() + " minutes and " + data.getUTCSeconds() + " second(s)";
}
// below function via: http://goo.gl/B3ae8c
function bytesToSize(bytes) {
    var k = 1000;
    var sizes = ['Bytes', 'KB', 'MB', 'GB', 'TB'];
    if (bytes === 0) return '0 Bytes';
    var i = parseInt(Math.floor(Math.log(bytes) / Math.log(k)), 10);
    return (bytes / Math.pow(k, i)).toPrecision(3) + ' ' + sizes[i];
}