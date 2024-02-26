// api.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ApiService {
  private apiUrl = '/file';

  constructor(private http: HttpClient) {}

  uploadFile(fileUploadModel: any): Observable<any> {
    const formData = new FormData();
    formData.append('file', fileUploadModel.file);
    formData.append('userEmail', fileUploadModel.userEmail);

    return this.http.post(`${this.apiUrl}/upload`, formData);
  }
}