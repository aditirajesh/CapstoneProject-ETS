import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms'; // Import FormsModule for ngModel
import { UserService } from '../../../services/user.service';
import { AuthService } from '../../../services/auth.service';
import { User } from '../../../models/UserModel';
import { CurrentUser } from '../../../models/CurrentUser';

@Component({
  selector: 'app-analyser-home',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule], // Add FormsModule here
  templateUrl: './analyser-home.html',
  styleUrls: ['./analyser-home.css']
})
export class AnalyserHomeComponent implements OnInit {
  // User and state management
  allUsers: User[] = [];
  filteredUsers: User[] = [];
  isLoading = true;
  error = '';
  currentUser: CurrentUser | null;

  // Search
  searchQuery = '';
  private searchDebounceTimer: any;

  // Stats
  totalUsersToAnalyse = 0;
  recentlyJoinedUsers = 0;

  constructor(
    private userService: UserService,
    public authService: AuthService,
    private router: Router
  ) {
    this.currentUser = this.authService.getCurrentUser();
  }

  ngOnInit(): void {
    this.loadUsers();
  }

  get displayUsername(): string {
    return this.currentUser?.username.split('@')[0] ?? 'Analyser';
  }

  loadUsers(): void {
    this.isLoading = true;
    this.error = '';
    this.userService.getAllUsers().subscribe({
      next: (users: User[]) => {
        // Filter out Admins and the analyser themselves
        this.allUsers = users.filter(u => 
          u.role !== 'Admin' && u.username !== this.currentUser?.username
        );
        this.filteredUsers = [...this.allUsers];
        this.calculateStats();
        this.isLoading = false;
      },
      error: (err: any) => {
        this.error = 'Failed to load users. Please try again later.';
        this.isLoading = false;
        console.error(err);
      }
    });
  }

  refreshUsers(): void {
    this.loadUsers();
  }

  calculateStats(): void {
    this.totalUsersToAnalyse = this.allUsers.length;
    
    const sevenDaysAgo = new Date();
    sevenDaysAgo.setDate(sevenDaysAgo.getDate() - 7);
    this.recentlyJoinedUsers = this.allUsers.filter(u => new Date(u.createdAt) >= sevenDaysAgo).length;
  }
  
  // --- Search and Filter Logic ---
  onSearchInputChange(): void {
    clearTimeout(this.searchDebounceTimer);
    this.searchDebounceTimer = setTimeout(() => {
      this.filterUsers();
    }, 300); // Debounce time of 300ms
  }

  filterUsers(): void {
    const query = this.searchQuery.toLowerCase().trim();
    if (!query) {
      this.filteredUsers = [...this.allUsers];
      return;
    }
    this.filteredUsers = this.allUsers.filter(user =>
      user.username.toLowerCase().includes(query) 
    );
  }

  clearSearch(): void {
    this.searchQuery = '';
    this.filteredUsers = [...this.allUsers];
  }

  // --- Navigation ---
  navigateToUserReports(username: string): void {
    this.router.navigate(['/reports'], {
      queryParams: { supervisoryView: 'true', targetUser: username }
    });
  }

  // ADDED: Navigation methods for the new navbar items
  navigateToMyExpenses(): void {
    this.router.navigate(['/expenses']);
  }

  navigateToMyReports(): void {
    this.router.navigate(['/reports']);
  }

  navigateToProfile(): void {
    this.router.navigate(['/profile']);
  }
  
  logout(): void {
    this.authService.logout();
  }

  // --- UI Helpers ---
  isUserOnline(lastSeen: Date): boolean {
    const fiveMinutes = 5 * 60 * 1000;
    return (new Date().getTime() - new Date(lastSeen).getTime()) < fiveMinutes;
  }
  
  formatLastSeen(dateString: string): string {
    const date = new Date(dateString);
    const now = new Date();
    const diff = now.getTime() - date.getTime();
    const diffDays = Math.floor(diff / (1000 * 3600 * 24));

    if (diffDays < 1) return 'Today';
    if (diffDays === 1) return 'Yesterday';
    return `${diffDays} days ago`;
  }
  
  formatJoinDate(date: Date): string {
    return new Date(date).toLocaleDateString('en-GB', {
      day: '2-digit',
      month: 'short',
      year: 'numeric'
    });
  }

  getUserRoleBadgeClass(role: string): string {
    switch (role.toLowerCase()) {
      case 'admin': return 'role-badge admin';
      case 'analyser': return 'role-badge analyser';
      case 'user': return 'role-badge user';
      default: return 'role-badge default';
    }
  }

  trackByUsername(index: number, user: User): string {
    return user.username;
  }
}