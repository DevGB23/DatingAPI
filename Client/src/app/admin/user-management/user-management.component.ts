import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService, ModalOptions } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/User';
import { UserWithRoles } from 'src/app/_models/UserWithRoles';
import { AdminService } from 'src/app/_services/admin.service';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit{
  users: UserWithRoles[] = [];
  bsModalRef: BsModalRef<RolesModalComponent> = new BsModalRef<RolesModalComponent>();
  availableRoles = ['Admin', 'Moderator', 'Member'];

  
  constructor(private adminService: AdminService, private modal: BsModalService){}
  
  ngOnInit(): void {
    this.getUsersWithRoles();
  }


  getUsersWithRoles() {
    this.adminService.getUsersWithRoles()
    .subscribe({
      next: users => {
        if (users) {
          this.users = users;
        }
      }
    })
  }


  openRolesModal(user: UserWithRoles) {
    const config = {
        class: 'model-dialog-centered',
        initialState: {
          username: user.username,
          availableRoles: this.availableRoles,
          selectedRoles: [...user.role]
        }
      }
    

    this.bsModalRef = this.modal.show(RolesModalComponent, config);
    this.bsModalRef.onHidden?.subscribe({
      next: () => {
        const selectedRoles = this.bsModalRef.content?.selectedRoles;
        if (!this.arrayEqual(selectedRoles!, user.role)) {
          this.adminService.updateUserRoles(user.username, selectedRoles!)
          .subscribe({
            next: roles => user.role = roles
          })
        }
      }
    })
  }


  private arrayEqual(arr1: any[], arr2: any[]) {
    return JSON.stringify(arr1.sort()) === JSON.stringify(arr2.sort())
  }


}
