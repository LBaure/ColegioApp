import {
  Component,
  ElementRef,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
  ViewChild
} from '@angular/core';
import { DomSanitizer } from '@angular/platform-browser';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Observable } from 'rxjs';
import { ViewFileComponent } from '../view-file/view-file.component';
import Swal from 'sweetalert2';

@Component({
  selector: 'file-upload',
  templateUrl: './file-upload.component.html',
  styleUrls: ['./file-upload.component.css']
})
export class FileUploadComponent implements OnInit, OnDestroy  {
  @ViewChild('file') inputFile: ElementRef<HTMLInputElement> = {} as ElementRef;

  @Input() required : boolean = false;
  @Input() label : string = "Subir Archivo"
  @Input() iconUpload : string = "bi bi-cloud-upload";
  @Input() iconShowFile : string = "bi bi-image";
  @Input() iconDeleteFile : string = "bi bi-trash";
  @Input() multiple : boolean = false;
  @Input() classBtnUpload : string = 'btn-info'
  @Output() model = new EventEmitter<File[]>();
  @Input() accept:string="application/pdf";
  @Input() colorModalViewFile: string = "dark";

  @Input() file = new Observable<any>(); // initialize observable with type any

  listFiles: any[] = [];
  identificador : string = '';
  modalViewFile?:BsModalRef;
  modelFile:string = '';
  alertViewFile: boolean = false;
  iconView : string = 'bi bi-list-task';
  viewList : boolean = false;

  private verb = 'initialized';
  constructor(
    public sanitizer: DomSanitizer,
    private modalService: BsModalService
  ) { this.identificador = this.generarGuid();}

  ngOnInit(): void {
    this.init('initialize');
  }

  init(msg : string) {
    console.log("method: ", msg);

  }


  selectFile () {
    document.getElementById(this.identificador)?.click()
  }

  /**
   * selectFiles
   * Metodo para seleccionar archivos.
   */
  selectFiles (files:any) {
    /**
     * VALIDACION
     * Si se ha seleccionado uno o mas archivos y estan guardados en memoria
     * y si buscamos un nuevo archivo y presionamos el boton cancelar
     *
     */
    if (this.listFiles.length && !files.length) {
      return
    }

    // Si el componente es simple, es decir que no acepta multiples archivos, se le asignara al
    // listado de archivos el valor [] vacio. para que solo agregue el entrante a la lista.
    if (!this.multiple) { this.listFiles = []}

    let lst = [...files]
    let noAccept : string[] = [];

    lst.forEach(element => {
      let valid = this.validateFile(element.type);
      // Se valida si existe una archivo con el mismo nombre
      let existFile = this.listFiles.find(file => file.name === element.name);

      if (valid) {
        if (!existFile) {
          const url = URL.createObjectURL(element);
          element.path = this.sanitizer.bypassSecurityTrustResourceUrl(url);
          this.listFiles.push(element)
        }
      } else {
        noAccept.push(element.name)
      }
    });

    if (noAccept.length) {
      let messagehtml : string = (noAccept.length === 1) ? "No se pudo cargar el archivo, no cumple con las extensiones permitidas" : 'No se pudieron cargar algunos archivos, no cumplen con las extensiones permitidas';
      Swal.fire({
        title: '<strong>Advertencia</strong>',
        icon: 'warning',
        iconColor: "var(--bs-warning)",
        html: messagehtml + this.getExtencionNoAcceptAlert(noAccept),
        confirmButtonColor: 'var(--bs-primary)',
        confirmButtonText:
          '<i class="bi bi-hand-thumbs-up"></i> Aceptar!',
      })
    }

    this.modelFile = this.getNameFiles();
    this.model.emit(this.listFiles)
  }

  private getNameFiles () : string {
     // Actualizar el elemento input Nombre de los archivos
     let stringName : string = '';
     // Se recorre la lista de archivos, despues de eliminar el archivo seleccionado
     this.listFiles.forEach(element => {
       stringName += element.name + "; "
     });
     return stringName;
  }


  getExtencionNoAcceptAlert(noAccept:any) {
    let str : string = ""
    noAccept.forEach((element : any) => {
      str += `<li>${element}</li>`
    })
    return `<ul class="text-muted mt-2 fs-12 text-start">${str}</ul>`;
  }

  /**
   * validateFile()
   * typeFile : string => recibe el tipo de extension del archivo
   * Validar las extensiones permitas para la carga del archivo
   */
  validateFile (typeFile : string) : boolean {
    const accept = this.accept.split(",");

    for (let i = 0; i < accept.length; i++) {
      const element = accept[i];
      if (element.trim() === typeFile.trim()) {
        return true;
      }

      // Validar cuando envian en la propiedad accept="image/*"
      // significa que acepta cualquier tipo de formato de imagen (jpg, jpeg, png)
      if (element.trim() === 'image/*') {
        // Se extrae la longitud del tipo antes del caracter /
        const len = typeFile.indexOf('/')
        // retornamos parte de la cadena de la variable TIPO desde la posicion 0 hasta el final de la variable anterior.
        const formato = typeFile.substring(0, len)
        return (formato === 'image') ? true : false;

      }

      // Validar cuando envian en la propiedad accept="video/*"
      // significa que acepta cualquier tipo de formato de video (jpg, jpeg, png)
      if (element.trim() === 'video/*') {
        const vlen = typeFile.indexOf('/')
        const vformato = typeFile.substring(0, vlen)
        return (vformato === 'image') ? true : false;
      }
    }
    return false;
  }


  deleteFile(file : File) {
    let result  = this.listFiles.findIndex(item => item.name === file.name )
    this.listFiles.splice(result, 1);
    this.inputFile.nativeElement.value = '';
    this.modelFile = this.getNameFiles();
    this.model.emit(this.listFiles)
  }

  generarGuid() {
    var dt = new Date().getTime();
    var uuid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function(c) {
        var r = (dt + Math.random()*16)%16 | 0;
        dt = Math.floor(dt/16);
        return (c=='x' ? r :(r&0x3|0x8)).toString(16);
    });
    return uuid;
  }

  openViewFile(file : any) {
    let ext = file.name.split(".").pop();
    // if (ext === "xlsx" || ext === "docx") {
    //   this.alertViewFile = true;
    //   setTimeout(() => {
    //     this.alertViewFile = false
    //   }, 2000);
    //   return;
    // }


    const initialState = {
      path : file.path,
      nameFile: file.name,
      typeFile: file.type,
      colorModal: this.colorModalViewFile

    }
    this.modalViewFile = this.modalService.show(ViewFileComponent, { initialState, class: 'modal-dialog-centered'});
  }

  changeView() {
    this.viewList = !this.viewList;
    this.iconView = (this.viewList) ? 'bi bi-grid-3x3-gap' : 'bi bi-list-task';
  }

  deleteAllFiles() {
    Swal.fire({
      icon: "warning",
      title: '¿Desea eliminar Todos los archivos?',
      showCancelButton: true,
      confirmButtonText: 'Si',
      cancelButtonText: 'No',
      customClass: {
        cancelButton: 'btn-danger'
      }
    }).then((result) => {
      if (result.isConfirmed) {
        this.listFiles = [];
        this.inputFile.nativeElement.value = '';
        this.modelFile = this.getNameFiles();
        this.model.emit(this.listFiles)

        Swal.fire('Se han eliminado todos los archivos correctamente!', '', 'success')
      }
    })

  }


  getExtensionFile(file : any) {
    let ext = file.name.split(".").pop();

    let pathError : string = ""
    switch (ext) {
      case "pdf":
        pathError = "../../../../assets/logo/pdf-logo.png"
        break;
      case "xlsx":
        pathError = "../../../../assets/logo/xls-logo.png"
        break
      case "docx":
        pathError = "../../../../assets/logo/word-logo.png"
        break
      case "json":
        pathError = "../../../../assets/logo/json-logo.png"
        break
      default:
        pathError = "../../../../assets/logo/notepad-logo.png"
        break;
    }
    document.getElementById(file.name)?.setAttribute('src', pathError);
  }


  ngOnDestroy() {
    console.log("ondestroy");

   }

}



interface fileModel {
  files: any
}
