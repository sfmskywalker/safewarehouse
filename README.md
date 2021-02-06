# Safe Warehouse

Safe Warehouse is a Blazor PWA that I created for a good friend of mine who is in the business of consulting on warehouse safety protocols.
The app helps them create reports when visiting warehouses while taking inventory of damages, if any.

![Safe Warehouse demo](/docs/animation.gif)

## Functionality
When creating a report, the app asks the user to upload a map of the warehouse or area to position damages on.
A damage consists of basic information, such as a description, location, some images and required material to fix or replace the damage.

Reports can be exported to PDF that are then emailed to the customer.

## Technical Aspects

The app is created as a Blazor PWA, and can be run both on WebAssembly as well as on the Server. I use Server mode during development to ease debugging, but the app is run by the client as a WebAssembly app.

The app can run offline. The data is stored on the device (in the browser) using [IndexedDb](https://developer.mozilla.org/en-US/docs/Web/API/IndexedDB_API). When the user connects to the Internet, the app will synchronize data stored online with data stored offline.

## UI

The UI is created with [TailwindUI](http://tailwindui.com/).
