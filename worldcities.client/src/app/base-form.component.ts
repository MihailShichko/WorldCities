import { Component } from '@angular/core';
import { AbstractControl, FormControl, FormGroup } from '@angular/forms';

@Component({
  template: ' '
})
export abstract class BaseFormComponent {

  protected form!: FormGroup;

  getErrors(control: AbstractControl, displayName: string, custumMessages: {[key: string] : string} | null = null): string[] {
    var errors: string[] = [];
    Object.keys(control.errors || {}).forEach((key) => {
      switch (key) {
        case 'required':
          errors.push(`${displayName} ${custumMessages?.[key] ?? "is required."}`);
          break;
        case 'pattern':
          errors.push(`${displayName} ${custumMessages?.[key] ?? "contains invalid characters."}`);
          break;
        case 'isDupeField':
          errors.push(`${displayName} ${custumMessages?.[key] ?? "already exists: please choose another."}`);
          break;
        default:
          errors.push(`${displayName} ${custumMessages?.[key] ?? "is invalid."}`);
          break;
      }
    });

    return errors;
  }

  constructor() { }
}
