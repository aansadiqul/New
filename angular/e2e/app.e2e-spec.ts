import { ABDTemplatePage } from './app.po';

describe('ABD App', function() {
  let page: ABDTemplatePage;

  beforeEach(() => {
    page = new ABDTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
