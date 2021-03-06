﻿namespace API.Helpers
{
    enum ListENUM
    {
        LIST_ALREADY_MARKED_AS_FAVOURITE,
        LIST_NOT_FOUND,
        UNABLE_ADD_LIST,
        UNABLE_ADD_LIST_TO_FAVOURITES,
        UNABLE_UPDATE_LIST,
        UNABLE_UPDATE_LIST_USER_IS_NOT_CREATOR
    }

    enum FilmENUM
    {
        FILM_NOT_FOUND,
        INCORRECT_FILM_ID,
        UNABLE_ADD_FILM,
        UNABLE_UPDATE_FILM
    }

    enum RateENUM
    {
        UNABLE_ADD_RATE,
        UNABLE_DELETE_RATE,
        UNABLE_UPDATE_RATE
    }

    enum UserENUM
    {
        ACCOUNT_ACTIVATED,
        ACCOUNT_NOT_CONFIRMED,
        DIFFERENT_PASSWORDS,
        EMAIL_ALREADY_TAKEN,
        EMAIL_NOT_EXISTS,
        PASSWORD_CHANGED,
        PASSWORD_TOO_SHORT,
        ROLE_NOT_FOUND,
        ROLE_ALREADY_ADDED_FOR_USER,
        UNABLE_ADD_ROLE,
        UNABLE_CHANGE_EMAIL,
        UNABLE_CHANGE_PASSWORD,
        UNABLE_CONFIRM_MAIL,
        UNABLE_DELETE_USER,
        UNABLE_REGISTER,
        UNABLE_RESET_PASSWORD,
        USERNAME_ALREADY_TAKEN,
        USER_ID_AND_CODE_REQUIRED,
        USER_NOT_FOUND,
        WRONG_PASSWORD
    }
}