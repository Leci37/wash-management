import { Component, inject, OnInit } from '@angular/core';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogModule,
} from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { MatIconModule } from '@angular/material/icon';
import { PhotoVO } from '../../../core/models/Photo';

export interface PhotoDialogData {
  photo: PhotoVO;
}
@Component({
  selector: 'cm-confirm-photo-dialog',
  templateUrl: './confirm-photo-dialog.html',
  styleUrl: './confirm-photo-dialog.scss',
  imports: [MatDialogModule, TranslateModule, MatIconModule],
})
export class ConfirmPhotoDialog implements OnInit {
  public dialog = inject(MatDialog);
  public data: PhotoDialogData = inject(MAT_DIALOG_DATA);
  public photo: PhotoVO | null = null;

  ngOnInit(): void {
    this.photo = this.data.photo;
  }
}
