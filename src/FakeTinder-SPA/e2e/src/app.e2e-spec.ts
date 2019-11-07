import { AppPage } from './app.po';

describe('Login', () => {
  let page: AppPage;

  const wrongCredentias = {
    username: 'wrongname',
    password: 'wrongpasswd'
  };

  beforeEach(() => {
    page = new AppPage();
  });

  it('should display welcome message', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('Find your match');
  });

  it('when user trying to login with wrong credentials he should stay on “login” page', () => {
    page.navigateTo();
    page.fillCredentials(wrongCredentias);
    expect(page.getParagraphText()).toEqual('Find your match');
  });

  it('when user trying to login with right credentials he should not stay on “login” page', () => {
    page.navigateTo();
    page.fillCredentials();
    expect(page.getMatchesLink()).toEqual('Matches');
  });

  it('when user logs out he should go on “login” page', () => {
    page.logOut();
    expect(page.getParagraphText()).toEqual('Find your match');
  });
});
