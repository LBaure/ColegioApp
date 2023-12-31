import { Directive, ElementRef, Input } from '@angular/core';

@Directive({
  selector: '[focus]'
})
export class AutoFocusDirective {

  private focus = true;

    constructor(private el: ElementRef)
    {
    }

    ngOnInit()
    {
        if (this.focus)
        {
            window.setTimeout(() =>
            {
                this.el.nativeElement.focus();
            });
        }
    }

    @Input() set autofocus(condition: boolean)
    {
        this.focus = condition !== false;
    }

}
