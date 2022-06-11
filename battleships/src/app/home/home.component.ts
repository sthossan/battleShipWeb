import { Component, OnInit } from '@angular/core';
import { BlockUI, NgBlockUI } from 'ng-block-ui';
import { CommonService } from '../_services/common.service';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {
    @BlockUI() blockUI!: NgBlockUI;
    errorMessage: any | '';
    list = [];

    constructor(private commonService: CommonService) { }
    ngOnInit(): void {
        this.getList();
    }


    getList() {
        this.errorMessage = '';
        this.blockUI.start('loading...');
        this.commonService.getAsync('game/index').subscribe({
            next: res => this.list = res,
            error: err => console.error('An error occurred', err),
            complete: () => console.log('There are no more data.')
        });
        this.blockUI.stop();
    }

    hitEnemy(column: any) {
        this.errorMessage = '';
        this.blockUI.start('wait...');
        if (column.color === 'hit' || column.color === 'lost') {
            this.errorMessage = 'This coordinate already being shot.';
            this.blockUI.stop();
            return;
        }
        if (this.list[0]['isObliteratedAll'] || this.list[1]['isObliteratedAll']) {
            this.errorMessage = 'Game over. Please reload new game';
            this.blockUI.stop();
            return;
        }

        this.commonService.postAsync(`game/enemyshot/${column.name}`, this.list).subscribe({
            next: res => {
                this.list = [];
                this.list = res;
                this.errorMessage = this.list[0]['isObliteratedAll'] ? 'Opponent win' : this.list[1]['isObliteratedAll'] ? 'You Win' : '';
            },
            error: err => {
                this.errorMessage = err;
            },
            complete: () => console.log('There are no more data.')
        });



        setTimeout(() => {
        }, 1000000);

        this.blockUI.stop();
    }

}
