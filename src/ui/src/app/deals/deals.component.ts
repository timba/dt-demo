import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';

@Component({
  selector: 'app-deals',
  templateUrl: './deals.component.html'
})
export class DealsComponent implements OnInit {

  private hubConnection: HubConnection;

  private connextionId: string;

  private dealRecords: DealRecord[];

  public dealsData: DealsData;

  public errorMessage: string;

  public loading: boolean;

  constructor(readonly http: HttpClient, @Inject('BASE_URL') readonly baseUrl: string) {
  }

  async ngOnInit() {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.baseUrl + 'dealshub')
      .build();

    this.hubConnection.on("id", (connectionId: string) => {
      this.connextionId = connectionId;
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

    await this.hubConnection.start();
  }

  fileSelected(files: FileList) {
    if (files.length == 0) {
      return;
    }

    let file = files[0];

    let formData = new FormData();
    formData.append('connectionId', this.connextionId);
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
