import { HttpTransportType, HubConnection, HubConnectionBuilder } from "../node_modules/@microsoft/signalr/dist/esm/index";
import { HmCommandModel } from "./HmCommandModel";
import { HmPredefinedCommandModel } from "./HmPredefinedCommandModel";
import { HtHubListeners } from "./hthublisteners";
import { HtUnityInstance } from "./htunityinstance";
import { TextLogModel } from "./TextLogModel";

const hubTries = [5, 10, 15];

export class HtHub {

    private unity = new HtUnityInstance();
    private hub:HubConnection|null = null;
    private listeners:HtHubListeners|null = null;

    private url:string = '';
    private gameCode:string = '';

    constructor(){
    }

    public setVars(url:string, gameCode:string, unityInstance:any):void {
        this.url = url;
        this.gameCode = gameCode;

        if(unityInstance === undefined || unityInstance === null){
            console.error('UnityInstance is null or undefined')
        }
        this.unity.unity = unityInstance;
    }

    public connect():void {
        if(this.hub !== null){
            this.stop();
        }

        this.hub = new HubConnectionBuilder()
            .withUrl(this.url, {
                skipNegotiation: true,
                transport: HttpTransportType.WebSockets
            })
            .withAutomaticReconnect(hubTries)
            .build();

        this.hub.start().then(() => {
            if(this.hub === null){
                console.error('Hub connected but somehow hub variable is null...');
                return;
            }
            
            this.listeners = new HtHubListeners(this.hub, this.unity);
            this.registerOnCallbacks();

            console.log('CONNECTED TO HUB gamecode: ', '`' + this.gameCode + '`');
            this.hub.invoke('JoinGameAsPlayer', this.gameCode);
            const msg:TextLogModel = {
                from: 'A player',
                message: 'Has joined'
            };
            this.hub.invoke('PlayerSendLog', this.gameCode, msg);
        }).catch((err) => {
            console.error('Error trying to connect', err);
            this.unity.OnConnectionStatusChanged({
                failedToConnect: true
            });
            this.stop();
        });
    }

    public stop():void {
        if(this.hub === null){
            return;
        }

        this.listeners?.unregister();
        this.listeners = null;
        this.unregisterOnCallbacks();

        const localHub = this.hub;
        this.hub = null;

        localHub.stop().finally(() => {
            console.log('Hub connection closed OK');
        }).catch((err) => {
            console.warn('Error trying to close', err);
        });        
    }

    private registerOnCallbacks(){
        if(this.hub === null){
            console.error('Hub is null');
            return;
        }

        this.hub.on('PlayerReceiveHmCommand', (data) => {
            const xdata = data as HmCommandModel;
            this.unity.OnHmCommand(xdata);

            this.hub?.invoke('PlayerSendBackHmCommand', this.gameCode, xdata);
        });

        this.hub.on('PlayerReceiveHmCommandPredefined', (data) => {
            const xdata = data as HmPredefinedCommandModel;
            this.unity.OnHmPredefinedCommand(xdata);

            this.hub?.invoke('PlayerSendBackHmCommandPredefined', this.gameCode, xdata);
        });
    }

    private unregisterOnCallbacks(){
        try{
            this.hub?.off('PlayerReceiveHmCommand');
            this.hub?.off('PlayerReceiveHmCommandPredefined');
        }catch(error){
            console.error('Error at unregister callbacks', error);
        }
    }

    public dispose(): void {
        this.stop();
    }
}