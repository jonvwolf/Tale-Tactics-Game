//import { hello } from "./src/lib";

//alert('ok: ' + hello('me'));

//import _ from 'lodash';

function component() {
  const element = document.createElement('div');

  element.innerHTML = 'hi from myself';

  return element;
}

document.body.appendChild(component());