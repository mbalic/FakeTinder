import { browser, by, element } from 'protractor';

export class AppPage {
  private credentials = {
    username: 'admin',
    password: 'password'
  };

  navigateTo() {
    return browser.get('/');
  }

  getParagraphText() {
    return element(by.css('app-root h1')).getText();
  }

  getMatchesLink() {
    return element(by.css('app-root a#matches-link')).getText();
  }

  fillCredentials(credentials: any = this.credentials) {
    element(by.css('[name="username"]')).sendKeys(credentials.username);
    element(by.css('[name="password"]')).sendKeys(credentials.password);
    element(by.css('.btn-success')).click();
  }

  logOut() {
    element(by.css('#dropdown-menu')).click();
    element(by.css('#logout')).click();
  }

}
