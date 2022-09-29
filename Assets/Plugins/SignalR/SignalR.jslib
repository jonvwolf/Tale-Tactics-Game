mergeInto(LibraryManager.library, {

  Hello: function () {
    window.alert("Hello, world!");
  },

  AddNumbers: function (x, y) {
    return x + y;
  },
  
  ConnectJs: function(gameCode, url) {
	console.log('Called ConnectJs');
	window.HtConnect(UTF8ToString(gameCode), UTF8ToString(url));
  },
  
  StopJs: function() {
	console.log('Called StopJs');
	window.HtStop();
  },
  
  DisposeJs: function() {
	console.log('Called DisposeJs');
	window.HtDispose();
  },

});