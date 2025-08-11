import { CanDeactivateFn } from '@angular/router';
import { ConfirmDialog } from '../../shared/components/confirm-dialog/confirm-dialog';

/* El componente debe implementar la interfaz FormDirty para que el formulario a controlar no necesariamente se llame "form".
  La función isFormDirty() debe devolver si el formulario está sucio o no.

  Para poder mostrar el dialogo de confirmación hay que asegurarse de que el componente tenga MatDialog y TranslateService importados.
  En el constructor se deben llamar "dialog" y "translate". 
  Ejemplo:
  constructor(
  public dialog: MatDialog,
  private translate: TranslateService) { }
*/
export const confirmSavedChanges: CanDeactivateFn<any> = async (
  component,
  // currentRoute,
  // currentState,
  // nextState,
) => {
  if (!component.isFormDirty()) {
    return true;
  }

  const title = 'UNSAVED_CHANGES_TITLE';
  const text = 'UNSAVED_CHANGES_TEXT';

  return component.dialog
    .open(ConfirmDialog, {
      data: {
        title,
        text,
        btn_text: true,
        btn_accept_text: 'SALIR',
        btn_cancel_text: 'CANCEL',
      },
    })
    .afterClosed()
    .toPromise()
    .then((result: boolean) => result === true);
};

export interface FormDirty {
  isFormDirty(): boolean;
}
