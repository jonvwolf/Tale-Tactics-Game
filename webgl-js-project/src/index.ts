/*
	Add this bundle.js to index.html (unity index.html)
		-> <script src="bundle.js"></script>
	To get unity reference in index.html
		-> .then((unityInstance) => { window.HtUnityInstance = unityInstance;
*/
import { HtHub } from "./hthub";

console.log('HtHubJSLink Loaded');

let htHub = new HtHub();

(window as any).HtConnect = function(gameCode: string, url: string) {
  console.log('Called HtConnect v6', gameCode, url);
  (window as any).HtUnityInstance.SendMessage('JsLink', 'InvokeEvent', '/*TestType*/{}');

  htHub.setVars(url, gameCode, (window as any).HtUnityInstance);
  htHub.connect();
};

(window as any).HtStop = function() {
  console.log('Called HtStop');

  htHub.stop();
};

(window as any).HtDispose = function() {
  console.log('Called HtDispose');

  htHub.dispose();
};
