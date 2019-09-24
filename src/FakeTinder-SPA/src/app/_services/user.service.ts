import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { User } from '../_models/user';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  baseUrl = environment.apiUrl + 'users/';

  constructor(private http: HttpClient) {}

  getUsers(): Observable<User[]> {
    return this.http.get<User[]>(this.baseUrl);
  }

  getUser(id): Observable<User> {
    return this.http.get<User>(this.baseUrl + id);
  }

  updateUser(id: Number, user: User) {
    return this.http.put(this.baseUrl + 'id' + id, user);
  }

  setMainPhoto(userId: Number, id: Number) {
    // We send empty object {} because it's post request
    // and we need to send something
    return this.http.post(
      this.baseUrl + userId + '/photos' + id + '/setMain',
      {}
    );
  }

  deletePhoto(userId: Number, id: Number) {
    return this.http.delete(this.baseUrl + 'users/' + userId + '/photos/' + id);
  }
}
