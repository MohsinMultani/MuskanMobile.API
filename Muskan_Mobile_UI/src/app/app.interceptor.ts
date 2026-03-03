import { Injectable } from '@angular/core';
import { HttpInterceptor } from '@angular/common/http';
import { HttpRequest, HttpErrorResponse } from '@angular/common/http'
import { Observable, throwError } from 'rxjs'
import { HttpHandler } from '@angular/common/http'
import { HttpEvent } from '@angular/common/http'


import * as express from 'express';
import type { Request, Response, NextFunction } from 'express';

import { createProxyMiddleware } from 'http-proxy-middleware';
import type { Filter, Options, RequestHandler } from 'http-proxy-middleware';



@Injectable({
  providedIn: 'root'
})

export class AppInterceptor implements HttpInterceptor {

  baseURL = 'https://localhost:7269/api'

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    return next.handle(request.clone({
      url: this.baseURL + request.url,
    }))
  }
}