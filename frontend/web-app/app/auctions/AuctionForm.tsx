"use client";
import { Button, TextInput } from "flowbite-react";
import React from "react";
import { FieldValues, useForm } from "react-hook-form";
import Input from "../components/Input";

export default function AuctionForm() {
    const {
        register,
        control,
        handleSubmit,
        setFocus,
        formState: { isSubmitting, isValid, isDirty, errors },
    } = useForm();

    //fieldvalues is from react form package
    function onSubmit(data: FieldValues) {
        console.log(data);
    }
    return (
        //use handleSunmit for react form
        //use the 'register' to register a field to react form
        <form
            action=""
            className="flex flex-col mt-3"
            onSubmit={handleSubmit(onSubmit)}
        >
            <Input
                label="Make"
                name="make"
                control={control}
                rules={{ required: "Make is required" }}
            />
            <Input
                label="Model"
                name="model"
                control={control}
                rules={{ required: "Model is required" }}
            />

            <div className="flex justify-between">
                <Button outline color="gray">
                    Cancel
                </Button>
                <Button
                    isProcessing={isSubmitting}
                    //so you can only submit if all field are valid
                    // disabled={!isValid}
                    type="submit"
                    outline
                    color="success"
                >
                    Submit
                </Button>
            </div>
        </form>
    );
}
