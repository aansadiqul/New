import { Component, Injector } from '@angular/core';
import { finalize } from 'rxjs/operators';
import { appModuleAnimation } from '@shared/animations/routerTransition';
import {
    PagedListingComponentBase,
    PagedRequestDto
} from '@shared/paged-listing-component-base';
import {
    ContactTitleServiceProxy,
    ContactTitleDto,
    PagedResultDtoOfContactTitleDto
} from '@shared/service-proxies/service-proxies';

class PagedContactTitlesRequestDto extends PagedRequestDto {
    keyword: string;
}

@Component({
    templateUrl: './contact-titles.component.html',
    animations: [appModuleAnimation()],
    styles: [
        `
          mat-form-field {
            padding: 10px;
          }
        `
    ]
})
export class ContactTitlesComponent extends PagedListingComponentBase<ContactTitleDto> {
    contactTitles: ContactTitleDto[] = [];
    contactTitle: ContactTitleDto = new ContactTitleDto();
    keyword = '';
    isActive: any | null;
    isLoading:boolean = false;

    constructor(
        injector: Injector,
        private _contactTitleService: ContactTitleServiceProxy
    ) {
        super(injector);
        this.isActive = '';
    }

    list(
        request: PagedContactTitlesRequestDto,
        pageNumber: number,
        finishedCallback: Function
    ): void {

        request.keyword = this.keyword;

        this._contactTitleService
            .getAllAsync(request.maxResultCount, request.skipCount, request.keyword, this.isActive)
            .pipe(
                finalize(() => {
                    finishedCallback();
                })
            )
            .subscribe((result: PagedResultDtoOfContactTitleDto) => {
                this.contactTitles = result.items;
                this.showPaging(result, pageNumber);
            });
    }

    delete(): void {
    }

    refreshData(): void {
        this.isLoading = true;
        this._contactTitleService.updateTypes().subscribe(() => {
            this.isLoading = false;
            this.refresh();
            this.notify.info(this.l('Data has been refreshed successfully'));
        });
    }

    onChange(event: any, contactTitle: ContactTitleDto) {
        if (event.checked) {
            contactTitle.isActive = true;
        } else {
            contactTitle.isActive = false;
        }

        this._contactTitleService
            .update(contactTitle)
            .subscribe(() => {
                // this.refresh();
                // this.notify.info(this.l('Data Successfully Updated'));
            });
    }
}
