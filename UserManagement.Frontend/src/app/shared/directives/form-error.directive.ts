import {
  Directive,
  Input,
  TemplateRef,
  ViewContainerRef,
  OnInit,
  OnDestroy,
} from '@angular/core';
import { AbstractControl } from '@angular/forms';
import { Subscription } from 'rxjs';

@Directive({
  selector: '[appFormError]',
  standalone: true,
})
export class FormErrorDirective implements OnInit, OnDestroy {
  @Input() appFormError!: AbstractControl;
  @Input() errorMessage?: string;

  private subscription?: Subscription;
  private hasView = false;

  constructor(
    private templateRef: TemplateRef<any>,
    private viewContainer: ViewContainerRef
  ) {}

  ngOnInit(): void {
    this.subscription = this.appFormError.statusChanges.subscribe(() => {
      this.updateView();
    });

    // Initial check
    this.updateView();
  }

  ngOnDestroy(): void {
    this.subscription?.unsubscribe();
  }

  private updateView(): void {
    const shouldShow =
      this.appFormError.invalid &&
      this.appFormError.touched &&
      this.appFormError.errors;

    if (shouldShow && !this.hasView) {
      this.viewContainer.createEmbeddedView(this.templateRef);
      this.hasView = true;
    } else if (!shouldShow && this.hasView) {
      this.viewContainer.clear();
      this.hasView = false;
    }
  }
}
