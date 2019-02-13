import { Component, OnInit, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-deals',
  templateUrl: './deals.component.html'
})
export class DealsComponent implements OnInit {

  public dealsData: DealsData;

  public errorMessage: string;

  public loading: boolean;

  constructor(readonly http: HttpClient, @Inject('BASE_URL') readonly baseUrl: string) { }

  ngOnInit() {
  }

  fileSelected(files: FileList) {
    console.log(files);
    if (files.length == 0) {
      return;
    }

    let file = files[0];

    let formData = new FormData();
    formData.append('file', file, file.name);
    this.loading = true;

    this.http.post<DealsData>(this.baseUrl + 'api/deals/upload', formData).subscribe(result => {
      this.dealsData = result;
      this.errorMessage = null;
      this.loading = false;
    }, error => {
      this.dealsData = null;
      this.loading = false;
      if (error.status == 400 && error.error.message) {
        this.errorMessage = error.error.message;
      } else {
        console.log(error);
        this.errorMessage = "Unknown error occured when uploading your file: " + error.message;
      }
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
