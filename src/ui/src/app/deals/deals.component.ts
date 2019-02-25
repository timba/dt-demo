import { Component, OnInit, OnDestroy, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@aspnet/signalr';

@Component({
  selector: 'app-deals',
  templateUrl: './deals.component.html',
  styleUrls: ['./deals.component.css']
})
export class DealsComponent implements OnInit, OnDestroy {

  private hubConnection: HubConnection;

  private resolveConnectionId: (connectionId: string) => void;

  private connectionId: Promise<string>;

  private dealRecords: DealRecord[];

  public dealsData: DealsData;

  public errorMessage: string;

  public loading: boolean;

  constructor(readonly http: HttpClient, @Inject('BASE_URL') readonly baseUrl: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.baseUrl + 'dealshub')
      .build();
  }

  private initializeConnectionIdPromise() {
    this.connectionId = new Promise<string>((resolve, _reject) => {
      this.resolveConnectionId = resolve;
    });
  }

  private async ensureHubConnected() {
    console.debug("Ensuring HUB connected:");
    if (this.hubConnection.state != HubConnectionState.Connected) {
      console.debug("=> HUB connecting...");
      await this.hubConnection.start();
      console.debug("=> HUB connected");
    } else {
      console.debug("=> HUB already connected");
    };
  }

  async ngOnInit() {
    this.hubConnection.on("id", (connectionId: string) => {
      console.debug("Connection ID received");
      this.resolveConnectionId(connectionId);
      console.debug("Connection ID resolved");
    });

    this.hubConnection.on("start", () => {
      this.dealsData = null;
      this.dealRecords = [];
      this.errorMessage = null;
      this.loading = true;
    });

    this.hubConnection.on("deal", (deal: DealRecord) => {
      this.dealRecords.push(deal);
    });

    this.hubConnection.on("stat", (stat: MostSoldVehicle) => {
      this.loading = false;
      this.dealsData = {
        deals: this.dealRecords.slice(0, 30),
        mostSoldVehicle: stat
      };
    });

    this.hubConnection.on("error", (error: string) => {
      this.dealsData = null;
      this.loading = false;
      this.errorMessage = error;
    });

    this.hubConnection.onclose(_error => {
      this.initializeConnectionIdPromise();
    });

    this.initializeConnectionIdPromise();
  }

  async ngOnDestroy() {
    await this.hubConnection.stop();
  }

  async fileSelected(files: FileList) {
    if (files.length == 0) {
      return;
    }

    await this.ensureHubConnected();

    let file = files[0];

    let formData = new FormData();
    console.debug("Awaiting Connection ID...");
    formData.append('connectionId', await this.connectionId);
    console.debug("Connection ID awaited");
    formData.append('file', file, file.name);

    this.loading = true;
    this.errorMessage = null;
    this.dealsData = null;

    this.http.post<DealsData>(this.baseUrl + 'api/deals/upload', formData).subscribe(
      () => { },
      error => {
        this.dealsData = null;
        this.loading = false;
        console.error(error);
        this.errorMessage = "Unknown error occured when uploading your file: " + JSON.stringify(error.error);
      });
  }
}

interface MostSoldVehicle {
  name: string;
  count: number;
}

interface DealRecord {
  id: number;
  customerName: string;
  dealershipName: string;
  vehicle: string;
  price: string;
  date: string;
}

interface DealsData {
  deals: DealRecord[];
  mostSoldVehicle: MostSoldVehicle;
}
