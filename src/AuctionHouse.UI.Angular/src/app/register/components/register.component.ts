import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { first } from 'rxjs/operators';

import { AuthenticationService } from '../../core/services/authentication.service';

@Component({
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  registerForm: FormGroup;
  loading = false;
  returnUrl: string;
  error = '';

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private authenticationService: AuthenticationService
  ) { }

  ngOnInit(): void {
    this.registerForm = this.formBuilder.group({
      email: ['', [Validators.email, Validators.required]],
      username: ['', Validators.required],
      password: ['',  [Validators.minLength(6), Validators.required]],
    });
  }

   onSubmit() {
    this.error = '';

    if (this.registerForm.invalid) {
        return;
    }

    this.loading = true;

    this.authenticationService.register(
      this.registerForm.controls['email'].value, 
      this.registerForm.controls['username'].value, 
      this.registerForm.controls['password'].value)
      .pipe(first())
      .subscribe(
        () => {
          this.router.navigate(['/login']);
        },
        (error: string) => {
          this.error = error;
          this.loading = false;
        });
  }
}
