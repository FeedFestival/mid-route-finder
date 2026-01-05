import { HttpClient, HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { CoreModule } from './core/core.module';
import { SharedModule } from './shared/shared.module';

import { AppRoutingModule } from './app-routing.module';

// NG Translate
import { TranslateLoader, TranslateModule } from '@ngx-translate/core';
import { TranslateHttpLoader } from '@ngx-translate/http-loader';

import { HomeModule } from './home/home.module';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { MessageService } from 'primeng/api';
import { DialogService } from 'primeng/dynamicdialog';
import { ToastModule } from 'primeng/toast';
import { AppComponent } from './app.component';
import { NavbarComponent } from './layout/navbar/navbar.component';

// AoT requires an exported function for factories
const httpLoaderFactory = (http: HttpClient): TranslateHttpLoader =>
    new TranslateHttpLoader(http, './assets/i18n/', '.json');

@NgModule({
    declarations: [AppComponent],
    imports: [
        BrowserModule,
        BrowserAnimationsModule,
        FormsModule,
        HttpClientModule,
        CoreModule,
        SharedModule,
        HomeModule,
        AppRoutingModule,
        ToastModule,
        TranslateModule.forRoot({
            loader: {
                provide: TranslateLoader,
                useFactory: httpLoaderFactory,
                deps: [HttpClient],
            },
        }),
        //
        NavbarComponent,
    ],
    providers: [DialogService, MessageService],
    bootstrap: [AppComponent],
})
export class AppModule {}
