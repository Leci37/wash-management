import { Component, inject, OnInit } from '@angular/core';
import {
  MAT_DIALOG_DATA,
  MatDialog,
  MatDialogModule,
} from '@angular/material/dialog';
import { TranslateModule } from '@ngx-translate/core';
import { MatIconModule } from '@angular/material/icon';

export interface DialogData {
  title: string;
  text: string;
  btn_text: boolean;
  btn_accept_text: string;
  btn_cancel_text: string;
}

@Component({
  selector: 'cm-confirm-dialog',
  templateUrl: 'confirm-dialog.html',
  styleUrls: ['./confirm-dialog.scss'],
  standalone: true,
  imports: [MatDialogModule, TranslateModule, MatIconModule],
})
export class ConfirmDialog implements OnInit {
  public dialog = inject(MatDialog);
  public data: DialogData = inject(MAT_DIALOG_DATA);
  public title = '';
  public text = '';
  public btn_accept_text = 'ACCEPT';
  public btn_cancel_text = 'CANCEL';

  ngOnInit(): void {
    this.title = this.data.title;
    this.text = this.data.text;
    this.btn_accept_text = this.data.btn_accept_text || 'ACCEPT';
    this.btn_cancel_text = this.data.btn_cancel_text || 'CANCEL';
  }
}
