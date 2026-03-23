export interface UserLoginResponseDto {
  token: string;
  expiresIn: number;
  user: {
    id: string;
    email: string;
    userName: string;
  };
}
