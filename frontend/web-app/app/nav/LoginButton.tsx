"use client";
import { Button } from "flowbite-react";
import { signIn } from "next-auth/react";
import React from "react";

export default function () {
  return (
    //signin is part of next auth, redirect to is the url to go after loggin in
    <Button
      outline
      onClick={() =>
        signIn("id-server", { redirectTo: "/" }, { prompt: "login" })
      }
    >
      Login
    </Button>
  );
}
