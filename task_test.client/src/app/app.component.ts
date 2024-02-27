import { Component } from '@angular/core';
import { ApiService } from './load-service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  fileUploadModel = { userEmail: '', file: undefined as File | undefined };

  constructor(private apiService: ApiService) { }

  onFileSelected(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    this.fileUploadModel.file = inputElement.files?.[0];
  }

  onSubmit(): void {
    if (this.fileUploadModel.file && this.fileUploadModel.userEmail) {
      const reader = new FileReader();

      reader.onload = (e) => {
        const arrayBuffer = reader.result as ArrayBuffer;
        const uint8Array = new Uint8Array(arrayBuffer);
        const byteNumbers = Array.from(uint8Array);

        const base64String = btoa(String.fromCharCode.apply(null, byteNumbers));

        // Send base64String to the server using your apiService
        this.apiService.uploadFileBytes(base64String, this.fileUploadModel.userEmail).subscribe(
          (response: any) => {
            console.log('File uploaded successfully:', response);
          },
          (error: any) => {
            console.error('Error uploading file:', error);
          }
        );
      };

      reader.readAsArrayBuffer(this.fileUploadModel.file);
    } else {
      console.warn('Please select a file and enter an email.');
    }
  }

 
}
