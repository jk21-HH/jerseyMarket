export interface UserRegisterRequest {
  username: string;
  password: string;
}

export interface UserLoginRequest {
  username: string;
  password: string;
}

export interface UserResponse {
  userId: number;
  username: string;
}

export interface AccessTokenRefreshTokenResponse {
  accessToken: string;
  refreshToken: string;
}
