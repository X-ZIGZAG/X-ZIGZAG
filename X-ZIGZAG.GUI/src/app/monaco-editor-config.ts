import { NgModule } from '@angular/core';
import { NGX_MONACO_EDITOR_CONFIG } from 'ngx-monaco-editor-v2';

@NgModule({
  providers: [
    {
      provide: NGX_MONACO_EDITOR_CONFIG,
      useValue: {
        // Define global configuration here
        defaultOptions: {
          theme: 'vs-dark', // Default theme
          language: 'javascript', // Default language
        },
      },
    },
  ],
})
export class MonacoEditorConfigModule {}
