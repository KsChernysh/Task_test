import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HttpHeaders } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = '/filemanager';

  constructor(private http: HttpClient) { }

  uploadFile(fileUploadModel: any): Observable<any> {
    const formData = new FormData();
    formData.append('file', fileUploadModel.file);
    formData.append('userEmail', fileUploadModel.userEmail);

    // Log FormData entries
    formData.forEach((value, key) => {
      console.log(key + ', ' + value);
    });

    return this.http.post(`filemanager/uploadfilebytes`, formData);
  }
   public httpOptions = {
    headers: new HttpHeaders({
      'Content-Type': 'multipart/form-data', // Set your desired content type
    }),
  };
  uploadFileBytes(fileBytes: string, userEmail: string): Observable<any> {
    const formData = new FormData();
    formData.append('file', fileBytes);
    formData.append('userEmail', userEmail);

    return this.http.post(`${this.apiUrl}/uploadbytes`, formData,this.httpOptions);
  }

}
