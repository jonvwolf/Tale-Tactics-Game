import { HubConnection } from "../node_modules/@microsoft/signalr/dist/esm/index";
import { HtUnityInstance } from "./htunityinstance";

export class HtHubListeners {

    private dispose:boolean = false;

    constructor(private hub:HubConnection|null, private unity:HtUnityInstance|null){
        if(this.hub === null || this.unity === null ){
            console.error('Hub or unity is null');
            return;
        }

        this.hub.onreconnecting((err) => {
            if(this.dispose === true){
                return;
            }
            // This is called only once (the first retry)
            console.warn('Hub reconnecting', err);
            //this._eventHubChanged.next({hubChanged: HubChangedEnum.Reconnecting});
            this.unity?.OnConnectionStatusChanged({
                isReconnecting: true
            });
        });

        this.hub.onreconnected((connId) => {
            if(this.dispose === true){
                return;
            }

            this.unity?.OnConnectionStatusChanged({
                reconnected: true
            });
        });

        this.hub.onclose((err) => {
            if(this.dispose === true){
                return;
            }

            // This is called when it fails to reconnect or when calling disconnect
            console.warn('Error. Hub connection closed', err)
            
            this.unity?.OnConnectionStatusChanged({
                disconnected: true
            });
        });
    }

    public unregister():void{
        this.dispose = true;
        this.hub = null;
        this.unity = null;
    }

}