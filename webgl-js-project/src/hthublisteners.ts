import { HubConnection } from "../node_modules/@microsoft/signalr/dist/esm/index";
import { GameCodeModel } from "./GameCodeModel";
import { HtUnityInstance } from "./htunityinstance";
import { TextLogModel } from "./TextLogModel";

export class HtHubListeners {

    private dispose:boolean = false;

    constructor(private hub:HubConnection|null, private unity:HtUnityInstance|null, private gameCode:GameCodeModel){
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

            try{
                // TODO: this code is copied from hthub
                this.hub?.invoke('JoinGameAsPlayer', this.gameCode)
                .then(() => {
                    console.log('RECONNECTED TO HUB');
                    const msg:TextLogModel = {
                        from: 'A player',
                        message: 'Has rejoined'
                    };
                    this.hub?.send('PlayerSendLog', this.gameCode, msg)
                        .then(() => { console.log('Successfully sent Player log'); })
                        .catch((err) => {
                            console.warn('Error sending PlayerSendLog', err);
                        });
                })
                .catch((err) => {
                    console.error('Error invoking JoinGameAsPlayer', err);
                    // TODO: how to stop from here?
                });
            }catch(err){
                console.error('Error trying to invoke JoinGameAsPlayer', err);
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
        console.log('Unregister hub in hthublisteners');
        this.hub = null;
        this.unity = null;
    }

}