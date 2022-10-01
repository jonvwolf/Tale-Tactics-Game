import { ConnectionStatusChangedModel as ConnectionStatusChangedModel } from "./ConnectionStatusChanged";
import { HmCommandModel } from "./HmCommandModel";
import { HmPredefinedCommandModel } from "./HmPredefinedCommandModel";

export class HtUnityInstance {

    public unity:any;

    constructor(){

    }

    public OnHmCommand(model:HmCommandModel):void{
        const str = JSON.stringify(model);
        console.log('Ready to send to JsLink JSON string', str, model);
        this.unity.SendMessage('JsLink', 'InvokeEvent', '/*OnHmCommand*/' + str);
    }

    public OnHmPredefinedCommand(model:HmPredefinedCommandModel):void{
        const str = JSON.stringify(model);
        console.log('Ready to send to JsLink JSON string', str, model);
        this.unity.SendMessage('JsLink', 'InvokeEvent', '/*OnHmPredefinedCommand*/' + str);
    }

    public OnConnectionStatusChanged(model:ConnectionStatusChangedModel):void{
        const str = JSON.stringify(model);
        console.log('Ready to send to JsLink JSON string', str, model);
        this.unity.SendMessage('JsLink', 'InvokeEvent', '/*OnConnectionStatusChangedType*/' + str);
    }
}