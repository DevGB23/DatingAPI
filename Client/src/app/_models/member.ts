import { Photo } from "./Photo"

export interface Member {
    id: number
    username: string
    photoUrl: string
    age: number
    knownAs: string
    createdAt: string
    lastActive: string
    gender: string
    introduction: string
    lookingFor: string
    interests: string
    country: string
    state: string
    city: string
    photos: Photo[]
  }
  
