/*
	Add this bundle.js to index.html (unity index.html)
		-> <script src="bundle.js?id=001"></script>
    ** Don't forget to increase the id
	To get unity reference in index.html
		-> .then((unityInstance) => { window.HtUnityInstance = unityInstance;
*/
import { HtHub } from "./hthub";
import { JsCodeModel } from "./JsCodeModel";

console.log('HtHubJSLink Loaded');

let htHub = new HtHub();

(window as any).HtGetCode = function() {
  console.log('Called HtGetCode');

  const params:any = new Proxy(new URLSearchParams(window.location.search), {
    get: (searchParams, prop) => searchParams.get(prop as string),
  });
  // Get the value of "some_key" in eg "https://example.com/?some_key=some_value"
  //const value = params.some_key; // "some_value"
  const value = params.code as string; // "some_value"

  if(value && value.length > 0){
    const model:JsCodeModel = {
      code: value
    }; 
    const str = JSON.stringify(model);
    (window as any).HtUnityInstance.SendMessage('JsLink', 'InvokeEvent', '/*OnCode*/' + str);
  }else{
    console.error('Code query param is null or length 0')
  }
};

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
